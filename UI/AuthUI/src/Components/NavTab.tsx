import { useState } from "react";

export type Tab = {
    label: string;
    value: string;
};


type NavTabProps = {
    tabs: Tab[];
    activeTab: string;
    onTabChange: (tabValue: string) => void;
}

export function NavTabs({ tabs, activeTab, onTabChange }: NavTabProps) {
    const [active, setActive] = useState(activeTab || tabs[0].value);

    const handleClick = (tabValue: string) => {
        setActive(tabValue);
        if (onTabChange)
            onTabChange(tabValue);
    }
    return (
        <div className="flex border-b border-gray-300">
            {
                tabs.map(tab => (
                    <button
                        key={tab.value}
                        onClick={() => handleClick(tab.value)}
                        className={`px-4 py-2 text-sm font-medium ${active === tab.value ? "border-b-2 border-blue-500 text-blue-600" : "text-gray-500 hover:text-gray-700"}`}
                    >
                        {tab.label}
                    </button>
                ))
            }
        </div>
    )
}